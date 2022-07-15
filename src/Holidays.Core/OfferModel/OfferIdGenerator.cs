using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Holidays.Core.OfferModel;

internal class OfferIdGenerator
{
    public Guid Generate(Offer offer)
    {
        var objectBytes = GetObjectBytes(offer);

        using var md5 = MD5.Create();

        var hash = md5.ComputeHash(objectBytes);
        return new Guid(hash);
    }

    private static byte[] GetObjectBytes(Offer offer)
    {
        var hotelLength = Encoding.UTF8.GetByteCount(offer.Hotel);
        var destinationLength = Encoding.UTF8.GetByteCount(offer.Destination);
        var cityOfDepartureLength = Encoding.UTF8.GetByteCount(offer.CityOfDeparture);
        var websiteNameLength = Encoding.UTF8.GetByteCount(offer.WebsiteName);

        var bytesCount =
            hotelLength +
            destinationLength +
            sizeof(int) +
            sizeof(int) +
            cityOfDepartureLength +
            websiteNameLength;

        var bytes = new byte[bytesCount];

        var offset = 0;

        offset += Encoding.UTF8.GetBytes(offer.Hotel, 0, offer.Hotel.Length, bytes, offset);

        offset += Encoding.UTF8.GetBytes(offer.Destination, 0, offer.Destination.Length, bytes, offset);

        Unsafe.As<byte, int>(ref bytes[offset]) = offer.DepartureDate.DayNumber;
        offset += sizeof(int);

        Unsafe.As<byte, int>(ref bytes[offset]) = offer.Days;
        offset += sizeof(int);

        offset += Encoding.UTF8.GetBytes(offer.CityOfDeparture, 0, offer.CityOfDeparture.Length, bytes, offset);

        Encoding.UTF8.GetBytes(offer.WebsiteName, 0, offer.WebsiteName.Length, bytes, offset);

        return bytes;
    }
}
