using System.Collections.Generic;

namespace WhiskyClub.DataAccess.Repositories
{
    public interface INoteRepository
    {
        Models.Note GetNote(int noteId);

        List<Models.Note> GetAllNotes();

        List<Models.Note> GetNotesForWhisky(int whiskyId);

        Models.Note InsertNote(int whiskyId, int eventId, int memberId, string comment);

        bool UpdateNote(int noteId, int whiskyId, int eventId, int memberId, string comment);

        bool DeleteNote(int noteId);

        byte[] GetNoteImage(int noteId);

        bool UpdateNoteImage(int noteId, byte[] image);
    }
}
