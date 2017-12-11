using GSEmulator.Commands;
using GSEmulator.Model;
using GSEmulator.GSProtocol3;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using NLog;

namespace GSEmulator
{
    class Program
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();

        static Server server;
        static ReaderWriterLockSlim mutex;
        static UdpClient endPoint;

        static void Main(string[] args)
        {
            Options options;

            try
            {
                options = Options.FromCommandLine(args);
            }
            catch(Options.ParsingException ex)
            {
                LOGGER.Fatal(ex.Message);
                LOGGER.Info(Options.ShowUsage());
                Environment.Exit(-1);
                return;
            }

            server = createDefaultPRServer();
            mutex = new ReaderWriterLockSlim();
            try
            {
                endPoint = new UdpClient(new IPEndPoint(options.EmulatorIP, options.EmulatorPort));
            }
            catch (Exception ex)
            {
                LOGGER.Fatal("Unable to create UDP listener: " + ex.Message);
                Environment.Exit(-1);
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                Thread thread = new Thread(() => ReporterWorker(options.BattlefieldIP, options.BattlefieldPort))
                {
                    IsBackground = false  // make sure no thread keeps working even if this reached the end
                };
                thread.Start();
            }

            LOGGER.Info("GSEmulator ready at {0}:{1}!", options.EmulatorIP.ToString(), options.EmulatorPort);
            for (string line = Console.ReadLine(); line != null; line = Console.ReadLine())
            {
                try
                {
                    LOGGER.Trace("Parsing: '{0}'", line);
                    Command command = Parser.Parse(line);

                    mutex.EnterWriteLock();
                    command.Execute(ref server);
                }
                catch (Exception ex)
                {
                    LOGGER.Warn(ex.ToString());
                }
                finally { if (mutex.IsWriteLockHeld) mutex.ExitWriteLock(); }
            }


            LOGGER.Warn("Python PIPE was closed");

            // Closing UDP Client
            // Causes worker threads to exit their while loop
            endPoint.Close();

            LOGGER.Info("GSEmulator is shuting down...");
        }

        /// <summary>
        /// Worker thread to handle the connections
        /// </summary>
        /// <param name="bf2IP"></param>
        /// <param name="bf2Port"></param>
        static void ReporterWorker(IPAddress bf2IP, int bf2Port)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] datagram;


            try
            {
                while ((datagram = endPoint.Receive(ref RemoteIpEndPoint)) != null)
                {
                    LOGGER.Debug("Received request from {0}:{1}", RemoteIpEndPoint.Address, RemoteIpEndPoint.Port);
                    LOGGER.Trace("Received request: {0}", BitConverter.ToString(datagram));

                    // If its not valid redirect to battlefield 2
                    if (!IsPacketValid(datagram))
                    {

#if !NOPROXY
                        LOGGER.Debug("Redirecting request to BF2 server - {0}:{1}", bf2IP, bf2Port);
                        ProxyTo(datagram, RemoteIpEndPoint, bf2IP, bf2Port);
#else
                        LOGGER.Warn("Received invalid message");
#endif
                        continue;
                    }


                    // A normal request: FE FD 00 XX XX XX XX AA BB CC 00
                    // Where the XX is the timestamp we want
                    byte[] timestamp = datagram.Skip(3).Take(4).ToArray();

                    try
                    {
                        mutex.EnterReadLock();
                        Encoder encoder = new Encoder(server, timestamp);
                        List<Message> messages = encoder.Encode(true, true, true);
                        mutex.ExitReadLock();

                        foreach (Message msg in messages)
                        {
                            var reply = msg.GetBytes();
                            LOGGER.Debug("Sending reply to {0}:{1}", RemoteIpEndPoint.Address, RemoteIpEndPoint.Port);
                            LOGGER.Trace("Sending reply: {0}", BitConverter.ToString(reply));
                            endPoint.Send(reply, reply.Length, RemoteIpEndPoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        LOGGER.Warn(ex.ToString());
                        if (mutex.IsReadLockHeld)
                            mutex.ExitReadLock();
                    }
                }
            }
            catch
            {
                // We'll always end here when endpoint.close() is called
            }
            finally
            {
                LOGGER.Debug("Stopping reporter worker");
            }
        }

        /// <summary>
        /// Creates a temporary proxy between the client and the game server.
        /// </summary>
        /// <param name="datagram">The initial datagram that the client sent</param>
        /// <param name="remoteIpEndPoint">The ip from where the datagram originates from</param>
        /// <param name="bf2IP">the IPAddress of the server that we're proxing to</param>
        /// <param name="bf2Port">the port of the server that we're proxing to</param>
        private static void ProxyTo(byte[] datagram, IPEndPoint remoteIpEndPoint, IPAddress bf2IP, int bf2Port)
        {
            using (UdpClient redirect = new UdpClient())
            {
                IPEndPoint bf2Server = new IPEndPoint(bf2IP, bf2Port);
                // Redirects request to bf2 server

                redirect.Send(datagram, datagram.Length, bf2Server);

                byte[] serverReply;
                // Receive from bf2 server
                while ((serverReply = redirect.Receive(ref bf2Server)) != null)
                {
                    // redirects bf2 response to client
                    LOGGER.Trace("Redirecting response from game to client: {0}", BitConverter.ToString(serverReply));
                    endPoint.Send(serverReply, serverReply.Length, remoteIpEndPoint);
                }
            }
        }

        /// <summary>
        /// Checks if the received UDP datagram is a Gamespy's Query Request that should be processed by this emulator.
        /// </summary>
        /// <param name="datagram">The UDP payload</param>
        /// <returns>True if the given payload is a Gamespy's Query Request</returns>
        private static bool IsPacketValid(byte[] datagram)
        {
            // Needs to have 11 bytes; no more no less
            if (datagram.Length != 11)
                return false;

            // Needs to start with 0xFE
            if (datagram[0] != 0xFE)
                return false;

            // Second byte needs to be 0xFD
            if (datagram[1] != 0xFD)
                return false;

            //11th byte needs to have least significate byte to 1
            if ((datagram[10] & 1) != 1)
                return false;

            return true;
        }


        static Server createDefaultPRServer()
        {
            Server server = new Server();
            server.HostName = "A PR Server";
            server.GameName = "battlefield2";
            server.GameVersion = "1.5.3153-802.0";      // Well game won't get any more updates, so this will be the same forever and ever
            server.GameMode = "openplaying";            // BF2 always replies with "openplaying"
            server.OS = "win32";
            server.TKMode = "Punish";
            server.Fps = 36;
            return server;
        }
    }
}
