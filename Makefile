OBJ ?= cube.obj
OBJ_PATH = images/$(OBJ)

all:
	dotnet run -- $(OBJ_PATH)
