# Stupid Firewall Manager

This is a simple program, that read a configuration based json files to support a 
simple monitoring of opened port in a windows environment.

## Config File Sample and basic functioning

Configuration file is a simple collection of rules, where every rule has a 
name to identify the rule, then it had a UdpPort to listen for command
a related TCP controlled port and a secret.

```
{
  "Rules" : [
    {
      "Name": "Rdp",
      "UdpPort": 23457,
      "TcpPort": 3389,
      "Secret": "this_is_a_secret"
    }
  ]
}
```

Server program is called StupidFirewallManager.exe and its purpose is managing firewall rules according
to configuration file.

Running the program will add some firewall rules prefixed with _sfm_block_udp and _sfm_block_tcpstatic
that will BLOCK every incoming connection to the PC from all port except controlled UDP Ports.

**This meeans that upon running the program all TCP port are closed**. Remember this if you are running
this program in a machine where you are accessing with RDP because it can completely isolate the machine
from your network.

## Ask for opening port

You can run StupidFirewallManager.Client.exe to ask for port opening, you need to specify ip address
udp port and the secret. Client will contact the server sending an Udp packet to specified udp port
with the secret. If the secret correspond to the one configured in the server the program will create
a rule that will open related TCP controlled port for a certain amount of time (default 2 hours)
