namespace VarlikYönetimi.Core.Enums
{
    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        BMOnayBekliyor = 3,
        DirektorOnayBekliyor = 4,
        GMYOnayBekliyor = 5,
        GMOnayBekliyor = 6,
        FMOdemeTarihiBelirleme = 7,
        OdemeHazir = 8,
        AvansGeriOdenmeyiBekliyor = 9,
        HukukiIslemBaslatildi = 10,
        AvansTalebiSonlandirildi = 11,
        Completed = 12
    }
} 