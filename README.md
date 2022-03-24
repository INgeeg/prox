# prox

#install rdp in ubuntu

sudo apt update
sudo apt install xrdp
sudo systemctl enable --now xrdp
sudo ufw allow from any to any port 3389 proto tcp
ip a --- find ip/hostname of ubuntu


#install qemu client
sudo apt-get install qemu-guest-agent


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
auto lo
iface lo inet loopback

auto ens18
iface ens18 inet static
address 192.168.0.111
  
# Install OpenSSH Server
sudo apt-get install openssh-server

# Install Docker
sudo su
apt-get update
apt-get install -y docker.io

# Run the following commands before installing the Kube environemnt
apt-get update && apt-get install -y apt-transport-https curl
curl -s https://packages.cloud.google.com/apt/doc/apt-key.gpg | apt-key add -
cat <<EOF >/etc/apt/sources.list.d/kubernetes.list
deb http://apt.kubernetes.io/ kubernetes-xenial main
EOF
apt-get update
apt-get install -y kubelet kubeadm kubectl
  
apt-mark hold kubelet kubeadm kubectl     #not needed
apt-mark unhold kubelet kubeadm kubectl     #not needed
  
  
# Update the Kubernetes configuration
nano /etc/systemd/system/kubelet.service.d/10-kubeadm.conf
  
# Add the below after the last line
Environment="cgroup-driver=systemd/cgroup-driver=cgroupfs"
  
Add kmaster to knode in /etc/hosts file and vice versa
  
  
  
  
  
# On Master
sudo kubeadm init --pod-network-cidr=192.168.0.0/16 --apiserver-advertise-address=192.168.0.188 --ignore-preflight-errors='All'
// For starting a Calico CNI: 192.168.0.0/16 or For starting a Flannel CNI: 10.244.0.0/16
  
# Run the following commands as normal user
mkdir -p $HOME/.kube
sudo cp -i /etc/kubernetes/admin.conf $HOME/.kube/config
sudo chown $(id -u):$(id -g) $HOME/.kube/config


#-----------------------(1)END-----------------------------
#-------------------------------------------------------




#------------(2)Show icons and dock in Ubuntu RDP-------------------------
#-------------------------------------------------------
sudo apt-get install gnome-tweak-tool
gnome-tweaks

In our famous xrdp installation scripts, to overcome the authentication required popup box for managed color device, we basically create a file and save it as 45-allow-colord.pkla and place it in the following directory (you need admin rights to copy files in this location !!)

/etc/polkit-1/localauthority/50-local.d/

To bypass the system repositories refresh authentication popup box, we have to either create an addition file or update the file we have just created. 

To keep it simple, we have added all the necessary authorization rules in a single file (i.e.  /etc/polkit-1/localauthority/50-local.d/45-allow-colord.pkla).   The content of the file should be similar to the one below

```
[Allow Colord all Users]
Identity=unix-user:*
Action=org.freedesktop.color-manager.create-device;org.freedesktop.color-manager.create-profile;org.freedesktop.color-manager.delete-device;org.freedesktop.color-manager.delete-profile;org.freedesktop.color-manager.modify-device;org.freedesktop.color-manager.modify-profile
ResultAny=no
ResultInactive=no
ResultActive=yes

[Allow Package Management all Users]
Identity=unix-user:*
Action=org.debian.apt.*;io.snapcraft.*;org.freedesktop.packagekit.*;com.ubuntu.update-notifier.*
ResultAny=no
ResultInactive=no
ResultActive=yes
```


#-----------------------(2)END-----------------------------
#-------------------------------------------------------








#------------(100)Template-------------------------
#-------------------------------------------------------



#-----------------------(100)END-----------------------------
#-------------------------------------------------------
  
  
  
  
  
  
  
  
  sudo swapoff -a
sudo sed -i '/ swap / s/^/#/' /etc/fstab
# Reboot a machine after that.
kubeadm reset
kubeadm init --ignore-preflight-errors all
  
