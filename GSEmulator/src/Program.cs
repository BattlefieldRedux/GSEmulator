﻿using GSEmulator.Commands;
using GSEmulator.Model;
using GSEmulator.GSProtocol3;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace GSEmulator
{
    class Program
    {
        static Server server = EncoderTests.CreateServer(75);

        static ReaderWriterLockSlim mutex = new ReaderWriterLockSlim();
        static UdpClient endPoint = new UdpClient( 21995 );

        static void Main(string[] args)
        {
            Console.WriteLine("Starting emulator at door {0}", 21995);

            for (int i = 0; i < 5; i++)
            {
                Thread thread = new Thread(new ThreadStart(ReporterWorker));
                thread.Start();
            }

            for (string line = Console.ReadLine(); ; line = Console.ReadLine())
            {
                Command command = Parser.Parse(line);


                try
                {
                    mutex.EnterWriteLock();
                    command.Execute(ref server);
                }
                catch { }
                finally { mutex.ExitWriteLock(); }
            }
        }


        static void ReporterWorker()
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] datagram;
            while ( (datagram = endPoint.Receive(ref RemoteIpEndPoint)) != null ) {
                Console.WriteLine(String.Format("Received request from {0}:{1}", RemoteIpEndPoint.Address, RemoteIpEndPoint.Port));

                byte[] timestamp = datagram.Skip(3).Take(4).ToArray();
 
                try
                {
                    mutex.EnterReadLock();
                    Encoder encoder = new Encoder(server, timestamp);
                    List<Message> messages = encoder.Encode(true, true, true);
                    mutex.ExitReadLock();

                    foreach (Message msg in messages) {
                        var reply = msg.GetBytes();
                        Console.WriteLine(String.Format("Sending reply to {0}:{1}", RemoteIpEndPoint.Address, RemoteIpEndPoint.Port));
                        endPoint.Send(reply, reply.Length, RemoteIpEndPoint);
                    }
                }
                catch {
                    mutex.ExitReadLock();
                }
            }
        }
    }
}