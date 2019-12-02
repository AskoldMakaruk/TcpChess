import socket
import select

IP = "127.0.0.1"
PORT = 22832
BUFFER_SIZE = 1024

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((IP, PORT))
while(True):
    s.send(input().encode("utf-8"))
    print("C#: " + s.recv(BUFFER_SIZE).decode("utf-8"))
    print("Python:", end=" ")
