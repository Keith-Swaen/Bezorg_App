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





#is dit goed?
# Bezorg_App

Deze handleiding beschrijft hoe je de Bezorg_App correct verbindt met de backend API op je eigen machine.

---

## 1. Firewall instellen (Windows Defender)

Om verbinding te maken met de API op je eigen computer, moet je poort 5111 openzetten in de Windows Defender Firewall:

1. Open **Windows Defender Firewall**.
2. Klik op **Advanced settings**.
3. Ga naar **Inbound Rules** en klik op **New Rule**.
4. Kies **Port** en klik op **Next**.
5. Selecteer **Specific local ports** en vul `5111` in. Klik op **Next**.
6. Klik drie keer op **Next** om de standaardinstellingen te accepteren.
7. Geef de regel een naam (bijvoorbeeld `API 5111`) en klik op **Finish**.

---

## 2. API clonen en starten

1. Clone de API repository:
   ```sh
   git clone https://github.com/LeVints/DeliveryMinimalAPI
   ```
2. Open het project in Visual Studio.
3. Start de applicatie met het **http** launch-profiel.

---

## 3. Bezorg_App configureren

1. Open de map van **Bezorg_App**.
2. Pas in `appsettings.json` de waarde van `ApiBaseUrl` aan naar:
   ```json
   "ApiBaseUrl": "http://JOUW_IPV4:5111"
   ```
   Vervang `JOUW_IPV4` door het IPv4-adres van jouw computer (zonder haakjes).
3. Ga naar `Bezorg_App/Platforms/Android/xml/network_security_config.xml`.
4. Vul ook hier jouw eigen IPv4-adres in waar nodig.

---

## Opmerkingen
- Zorg dat je computer en je testapparaat (bijvoorbeeld je telefoon) in hetzelfde netwerk zitten.
- Je kunt je IPv4-adres vinden via `ipconfig` in de opdrachtprompt.
