# SecureChannelMessenger

SecureChannelMessenger is a client-server application developed as part of the Computer and Network Security, CS432, course at Sabancı University, utilizing C# socket programming for secure and authenticated message broadcasting. This application serves as a robust platform for subscribers across different channels to send and receive messages in a secure, authenticated manner. The project encompasses two main modules, the Server module and the Client module, intricately designed to ensure secure communications within the network.

## Project Overview

### Server Module
The Server module receives messages from clients and broadcasts them to the clients subscribed to a specific channel. The server manages client enrollments and initiates the secure environment.

### Client Module
The Client module registers to the Server, subscribes to a channel, and gets authenticated. Once authenticated, the client can securely send and receive messages through the subscribed channel.

## Features

- Secure enrollment and authentication of clients.
- Secure message broadcast mechanism.
- User-friendly and functional GUI for client and server programs.
- Extensive reporting of all activities and data in text fields on the GUI.
- Display of keys, challenges, responses, digital signatures, and other security elements in hexadecimal format.
- Display of verification results, message transfer operations, and message texts.
- Management of online clients from the server side.

## Team Members

- [Barış Rastgeldi](https://github.com/barisrast)
- [Mert Kılıçaslan](https://github.com/mertkilicaslan)


**Disclaimer**: This project is for educational purposes. The authors are not responsible for any potential security breaches resulting from its use.
