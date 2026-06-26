OBJ ?= 42.obj
OBJ_PATH = images/$(OBJ)

all:
	dotnet run -- $(OBJ_PATH)

build:
	dotnet build

clean:
	dotnet clean
	rm -rf bin/ obj/

fclean: clean
	rm -rf bin/ obj/

re: fclean all
