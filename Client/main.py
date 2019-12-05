import socket
import select


class Server:
    IP = "127.0.0.1"
    PORT = 22832
    BUFFER_SIZE = 1024

    Connection: socket

    def __init__(self):
        self.Connection = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.Connection.connect((self.IP, self.PORT))

    def sendMessage(self, mes: str):
        self.Connection.send((mes+"\n").encode("utf-8"))
        return self.Connection.recv(self.BUFFER_SIZE).decode("utf-8")


serv = Server()
while(True):
    print(serv.sendMessage(input("text: ")))
