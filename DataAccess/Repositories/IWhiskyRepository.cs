using System;
using System.Collections.Generic;
using WhiskyClub.DataAccess.Models;

namespace WhiskyClub.DataAccess.Repositories
{
    public interface IWhiskyRepository : IDisposable
    {
        Whisky GetWhisky(int whiskyId);

        List<Whisky> GetAllWhiskies();

        Whisky InsertWhisky(string name, string brand, int age, string country, string region, string description, decimal? price, int? volume);

        bool UpdateWhisky(int whiskyId, string name, string brand, int age, string country, string region, string description, decimal? price, int? volume);

        bool DeleteWhisky(int whiskyId);

        bool UpdateImage(int whiskyId, byte[] image);
    }
}
