# prox

#install rdp in ubuntu

sudo apt update
sudo apt install xrdp
sudo systemctl enable --now xrdp
sudo ufw allow from any to any port 3389 proto tcp
ip a --- find ip/hostname of ubuntu


#install qemu client
sudo apt-get install qemu-guest-install


#------------(1)PREPARE VM for K8S-------------------------
#-------------------------------------------------------



#-----------------------(1)END-----------------------------
#-------------------------------------------------------




#------------(2)Show icons and dock in Ubuntu RDP-------------------------
#-------------------------------------------------------
sudo apt-get install gnome-tweak-tool
gnome-tweaks
#-----------------------(2)END-----------------------------
#-------------------------------------------------------








#------------(100)Template-------------------------
#-------------------------------------------------------



#-----------------------(100)END-----------------------------
#-------------------------------------------------------
