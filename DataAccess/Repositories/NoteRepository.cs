using System;
using System.Collections.Generic;
using System.Linq;
using WhiskyClub.DataAccess.Entities;

namespace WhiskyClub.DataAccess.Repositories
{
    public class NoteRepository : EntityFrameworkRepositoryBase, INoteRepository
    {
        public Models.Note GetNote(int noteId)
        {
            var note = GetOne<Note, int>(noteId);

            return new Models.Note
            {
                NoteId = note.NoteId,
                WhiskyId = note.WhiskyId,
                EventId = note.EventId,
                Comment = note.Comment
            };
        }

        public List<Models.Note> GetAllNotes()
        {
            var items = from note in GetAll<Note>()
                        select new Models.Note
                        {
                            NoteId = note.NoteId,
                            WhiskyId = note.WhiskyId,
                            EventId = note.EventId,
                            Comment = note.Comment
                        };

            return items.ToList();
        }

        public List<Models.Note> GetNotesForWhisky(int whiskyId)
        {
            var items = from note in GetAll<Note>()
                        where note.WhiskyId == whiskyId
                        select new Models.Note
                        {
                            NoteId = note.NoteId,
                            WhiskyId = note.WhiskyId,
                            EventId = note.EventId,
                            Comment = note.Comment
                        };

            return items.ToList();
        }

        public Models.Note InsertNote(int whiskyId, int eventId, int memberId, string comment)
        {
            try
            {
                var note = new Note
                {
                    WhiskyId = whiskyId,
                    EventId = eventId,
                    MemberId = memberId,
                    Comment = comment,
                    InsertedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                Insert(note);

                CommitChanges();

                return new Models.Note
                {
                    NoteId = note.NoteId,
                    WhiskyId = note.WhiskyId,
                    EventId = note.EventId,
                    Comment = note.Comment
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateNote(int noteId, int whiskyId, int eventId, int memberId, string comment)
        {
            try
            {
                var note = GetOne<Note, int>(noteId);
                note.WhiskyId = whiskyId;
                note.EventId = eventId;
                note.MemberId = memberId;
                note.Comment = comment;
                note.UpdatedDate = DateTime.Now;

                Update(note);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteNote(int noteId)
        {
            try
            {
                var note = new Note { NoteId = noteId };

                Delete(note);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public byte[] GetNoteImage(int noteId)
        {
            var note = GetOne<Note, int>(noteId);

            return note.Image;
        }

        public bool UpdateNoteImage(int noteId, byte[] image)
        {
            try
            {
                var note = GetOne<Note, int>(noteId);
                note.Image = image;
                note.UpdatedDate = DateTime.Now;

                Update(note);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
