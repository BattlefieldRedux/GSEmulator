using GSEmulator.Commands;
using GSEmulator.Model;
using GSEmulator.GSProtocol3;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using GSEmulator.Util;

namespace GSEmulator
{
    class Program
    {
        static readonly string TAG = "Program";
        static Server server;
        static ReaderWriterLockSlim mutex;
        static UdpClient endPoint;

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Log.e(TAG, "Program started with wrong arguments: Wrong amount of args");
                throw new ArgumentOutOfRangeException("Expects two argumemnts, specifing the ip and port that it will listen from");
            }

            IPAddress ip;
            if(!IPAddress.TryParse(args[0], out ip )) {
                Log.e( TAG, "Program started with wrong arguments: Ip invalid" );
                throw new ArgumentException( "Ip invalid." );
            }

            ushort lPort;
            if (!ushort.TryParse( args[1], out lPort )) {
                Log.e( TAG, "Program started with wrong arguments: Port invalid" );
                throw new ArgumentException( "Port invalid." );
            }

            server = new Server();
            mutex = new ReaderWriterLockSlim();
            endPoint = new UdpClient(new IPEndPoint(ip, lPort));

            for (int i = 0; i < 5; i++)
            {
                Thread thread = new Thread(new ThreadStart(ReporterWorker));
                thread.Start();
            }

            Log.d(TAG, String.Format("GSEmulator ready at door {0}!", lPort));
            for (string line = Console.ReadLine(); ; line = Console.ReadLine())
            {
                Log.d(TAG, String.Format("Parsing: '{0}'", line));
                Command command = Parser.Parse(line);
                try
                {
                    mutex.EnterWriteLock();
                    command.Execute(ref server);
                }
                catch (Exception ex){
                    Log.w(TAG, ex.ToString());
                }
                finally { mutex.ExitWriteLock(); }
            }
        }

        static void ReporterWorker()
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] datagram;
            while ( (datagram = endPoint.Receive(ref RemoteIpEndPoint)) != null ) {
                Log.d(TAG, String.Format("Received request from {0}:{1}", RemoteIpEndPoint.Address, RemoteIpEndPoint.Port));

                byte[] timestamp = datagram.Skip(3).Take(4).ToArray();
 
                try
                {
                   
                    mutex.EnterReadLock();
                    Encoder encoder = new Encoder(server, timestamp);
                    List<Message> messages = encoder.Encode(true, true, true);
                    mutex.ExitReadLock();

                    foreach (Message msg in messages) {
                        var reply = msg.GetBytes();
                        Log.d(TAG, String.Format("Sending reply to {0}:{1}", RemoteIpEndPoint.Address, RemoteIpEndPoint.Port));
                        endPoint.Send(reply, reply.Length, RemoteIpEndPoint);
                    }
                }
                catch (Exception ex){
                    Log.w(TAG, ex.ToString());
                    mutex.ExitReadLock();
                }
            }
        }
    }
}
