#!/bin/bash
APP_NAME="RemoteHub"
EXECUTABLE="/usr/local/bin/RemoteHub"
INSTALL_DIR="/opt/$APP_NAME"
BIN_DIR="/usr/local/bin"
ICON_DIR="/usr/share/icons/hicolor/apps"
DESKTOP_FILE="/usr/share/applications/$APP_NAME.desktop"

# Créer le dossier d’installation
sudo mkdir -p "$INSTALL_DIR"
sudo cp $APP_NAME "$INSTALL_DIR/"
sudo chmod +x "$INSTALL_DIR/$APP_NAME"

# Lien symbolique dans /usr/local/bin
sudo ln -sf "$INSTALL_DIR/$APP_NAME" "$BIN_DIR/$APP_NAME"

# Installer l’icône
sudo mkdir -p "$ICON_DIR"
sudo cp Icone.png "$ICON_DIR/$APP_NAME.png"

# Créer le fichier .desktop
sudo tee "$DESKTOP_FILE" > /dev/null <<EOF
[Desktop Entry]
Name=$APP_NAME
Exec=$APP_NAME
Icon=$APP_NAME
Type=Application
Categories=Utility;
StartupNotify=true
EOF

# Mise à jour caches
sudo update-desktop-database /usr/share/applications
sudo gtk-update-icon-cache /usr/share/icons/hicolor

# Détection de la distribution
if [ -f /etc/os-release ]; then
    source /etc/os-release
    DISTRO=$ID
elif command -v lsb_release >/dev/null 2>&1; then
    DISTRO=$(lsb_release -si | tr '[:upper:]' '[:lower:]')
else
    echo "Impossible de détecter la distribution"
    exit 1
fi

echo "Distribution détectée : $DISTRO"

# Liste des dépendances communes (exemple)
DEPS="dotnet-sdk-8.0 dotnet-runtime-8.0 freerdp"

# Installation selon la distribution
case "$DISTRO" in
    ubuntu|debian)
        echo "Installation via apt..."
        sudo apt update
        sudo apt install -y $DEPS
        ;;
    fedora)
        echo "Installation via dnf..."
        sudo dnf install -y $DEPS
        ;;
    arch)
        echo "Installation via pacman..."
        sudo pacman -Sy --noconfirm $DEPS
        ;;
    opensuse*|suse)
        echo "Installation via zypper..."
        sudo zypper install -y $DEPS
        ;;
    *)
        echo "Distribution non supportée : $DISTRO"
        exit 1
        ;;
esac



