board = [[0]*8]*8
for row in board:
    for s in row:
        print(s, end=" ")
    print()


def populateBoard():    
    rook = Piece()
    board[0][0]


class Piece:
    x = 0
    y = 0
    white = True
    availibleMoves = [[0]*1]*2

    __init__(x: int, y: int, white: bool, availibleMoves: list):
        self.x = x
        self.y = y
        self.white = white
        self.availibleMoves = availibleMoves
