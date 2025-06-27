# Bezorg_App

Connectie met API starten:

1. Firewall defender firewall inbound rule aanpassen
1.1 Open firewall defender firewall
1.2 klik op advanced settings, klik dan op inbound rules en daarna new rue
1.3 klik op port, next, vul bij specific local ports 5111 in, next, next, next, bij name wat je ook wil

2. Clone de API https://github.com/LeVints/DeliveryMinimalAPI
2.1 Start applicatie met http

3. Open Bezorg_App
3.1 Pas bij appsettings.json de ApiBaseUrl aan naar "http://(JOUW IPv4):5111" // zonder ()
3.2 Ga naar Bezorg_App/Platforms/Android/xml/network_security_config.xml en schrijf bij IPv4 jouw eigen IPv4
