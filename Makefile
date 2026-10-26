OBJ ?= 42.obj
OBJ_PATH = images/$(OBJ)
PROJECT     = scop.csproj
DOTNET_DIR  = $(HOME)/.dotnet
DOTNET      = $(shell command -v dotnet 2>/dev/null || echo $(DOTNET_DIR)/dotnet)

all:	deps $(NAME)
	dotnet run -- $(OBJ_PATH)

deps:
	@command -v dotnet >/dev/null 2>&1 || [ -x $(DOTNET_DIR)/dotnet ] || \
	( echo "→ .NET SDK introuvable, installation dans $(DOTNET_DIR)..." && \
		curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 8.0 --install-dir $(DOTNET_DIR) )
	@echo "→ Restauration des paquets NuGet..."
	@$(DOTNET) restore $(PROJECT)
	@echo "✔ Dépendances OK"

build:
	dotnet build

clean:
	dotnet clean
	rm -rf bin/ obj/

fclean: clean
	rm -rf bin/ obj/

re: fclean all
