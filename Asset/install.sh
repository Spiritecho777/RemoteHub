#!/bin/bash
APP_NAME="RemoteHub"
EXECUTABLE="/usr/local/bin/RemoteHub"


sudo ln -s /opt/Stusoft/RemoteHub /usr/local/bin/remotehub


#script d'installtion 
# copier les fichier ou il faut
# creer le .desktop
#installer les dependances

================================================================================================================
#!/bin/bash

APP_NAME="RemoteHub"
INSTALL_DIR="/opt/$APP_NAME"
BIN_DIR="/usr/local/bin"
ICON_DIR="/usr/share/icons/hicolor/256x256/apps"
DESKTOP_FILE="/usr/share/applications/$APP_NAME.desktop"

echo "📦 Installation de $APP_NAME..."

# Créer le dossier d’installation
sudo mkdir -p "$INSTALL_DIR"
sudo cp bin/$APP_NAME "$INSTALL_DIR/"
sudo chmod +x "$INSTALL_DIR/$APP_NAME"

# Lien symbolique dans /usr/local/bin
sudo ln -sf "$INSTALL_DIR/$APP_NAME" "$BIN_DIR/$APP_NAME"

# Installer l’icône
sudo mkdir -p "$ICON_DIR"
sudo cp assets/Icone.png "$ICON_DIR/$APP_NAME.png"

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

echo "✅ $APP_NAME installé. Lancez-le depuis le menu ou avec '$APP_NAME'."
================================================================================================================