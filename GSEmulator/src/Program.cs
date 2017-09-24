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

            LOGGER.Info("Starting GSEmulator with following arguments: {0}", string.Join(", ", args));

            if (args.Length != 2)
            {
                LOGGER.Fatal("Program started with wrong arguments: Expexted 2 got {0}", args.Length);
                Environment.Exit(-1);
                return;
            }

            IPAddress ip;
            if (!IPAddress.TryParse(args[0], out ip))
            {
                LOGGER.Fatal("Program started with wrong arguments: Ip invalid got '{0}'", args[0]);
                Environment.Exit(-1);
                return;
            }

            ushort lPort;
            if (!ushort.TryParse(args[1], out lPort))
            {
                LOGGER.Fatal("Program started with wrong arguments: Port invalid got '{0}'", args[1]);
                Environment.Exit(-1);
                return;
            }

            server = createDefaultPRServer();
            mutex = new ReaderWriterLockSlim();
            try
            {
                endPoint = new UdpClient(new IPEndPoint(ip, lPort));
            }
            catch (Exception ex) {
                LOGGER.Fatal("Unable to create UDP listener: " + ex.Message);
                Environment.Exit(-1);
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                Thread thread = new Thread(new ThreadStart(ReporterWorker))
                {
                    IsBackground = false  // make sure no thread keeps working even if this reached the end
                };
                thread.Start();
            }

            LOGGER.Info("GSEmulator ready at {0}:{1}!", ip.ToString(), lPort);
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
                    LOGGER.Warn( ex.ToString());
                }
                finally { if(mutex.IsWriteLockHeld) mutex.ExitWriteLock(); }
            }


            LOGGER.Warn("Python PIPE was closed");

            // Closing UDP Client
            // Causes worker threads to exit their while loop
            endPoint.Close();

            LOGGER.Info("GSEmulator is shuting down...");
        }

        static void ReporterWorker()
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] datagram;


            try
            {
                while ((datagram = endPoint.Receive(ref RemoteIpEndPoint)) != null)
                {
                    LOGGER.Debug("Received request from {0}:{1}", RemoteIpEndPoint.Address, RemoteIpEndPoint.Port);
                    LOGGER.Trace("Received request: {0}", BitConverter.ToString(datagram));

                    // Its GSDiverter's job to make sure only good requests are diverted to us
                    if (datagram.Length < 7)
                    {
                        LOGGER.Warn("Received request with invalid size");
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
            finally {
                LOGGER.Debug("Stopping reporter worker");
            }
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
