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

# Only on the Master
# Update the repositporles
sudo su
apt-get update

# 11 Turn off Swap Space
swapoff -a
nano /etc/fstab       #comment out swapfile line

# Update the Hostname
nano /etc/hostname

# Note the IP address
ifconfig

# Update the hosts file
nano /etc/hosts

# Set a static IP address
nano /etc/network/interfaces

# Add the below at the end of the file
auto enp0s8
iface ep0s8 inet static
address <IP-Address-Of-VM>
  
# Install OpenSSH Server
sudo apt-get install openssh-server

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
