using System;
using System.Collections.Generic;
using WhiskyClub.DataAccess.Models;

namespace WhiskyClub.DataAccess.Repositories
{
    public interface ITastingNoteRepository
    {
        Models.TastingNote GetTastingNote(int tastingNoteId);

        List<Models.TastingNote> GetAllTastingNotes();

        List<Models.TastingNote> GetTastingNotesForWhisky(int whiskyId);

        Models.TastingNote InsertTastingNote(int whiskyId, int eventId, int memberId, string comment);

        bool UpdateTastingNote(int tastingNoteId, int whiskyId, int eventId, int memberId, string comment);

        bool DeleteTastingNote(int tastingNoteId);

        byte[] GetTastingNoteImage(int tastingNoteId);

        bool UpdateTastingNoteImage(int tastingNoteId, byte[] image);
    }
}
