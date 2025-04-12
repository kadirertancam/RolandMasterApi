using Sanford.Multimedia.Midi;

try
{
    // 0 indeksi ile MIDI çıkış cihazını açıyoruz (ilk cihaz)
    using (OutputDevice outDevice = new OutputDevice(1))
    {
        // "B0 31 7F" mesajını oluşturuyoruz:
        // - B0: Control Change mesajı, kanal 1 (0 indeksten itibaren)
        // - 0x31: Kontrol numarası (49)
        // - 0x7F: Değer (127)
        ChannelMessage controlMessage = new ChannelMessage(ChannelCommand.Controller, 0, 0x31, 0x7F);

        // Oluşturulan kontrol mesajını gönderiyoruz
        outDevice.Send(controlMessage);
    }
}
catch (Exception ex)
{
    Console.WriteLine("Hata: " + ex.Message);
}