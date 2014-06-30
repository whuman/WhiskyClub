using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhiskyClub.DataAccess.Entities;

namespace WhiskyClub.DataAccess.Repositories
{
    public class TastingNoteRepository : EntityFrameworkRepositoryBase, ITastingNoteRepository
    {
        public Models.TastingNote GetTastingNote(int tastingNoteId)
        {
            var tastingNote = GetOne<TastingNote, int>(tastingNoteId);

            return new Models.TastingNote
            {
                TastingNoteId = tastingNote.TastingNoteId,
                WhiskyId = tastingNote.WhiskyId,
                EventId = tastingNote.EventId,
                Comment = tastingNote.Comment
            };
        }

        public List<Models.TastingNote> GetAllTastingNotes()
        {
            var items = from tastingNote in GetAll<TastingNote>()
                        select new Models.TastingNote
                        {
                            TastingNoteId = tastingNote.TastingNoteId,
                            WhiskyId = tastingNote.WhiskyId,
                            EventId = tastingNote.EventId,
                            Comment = tastingNote.Comment
                        };

            return items.ToList();
        }

        public List<Models.TastingNote> GetTastingNotesForWhisky(int whiskyId)
        {
            var items = from tastingNote in GetAll<TastingNote>()
                        where tastingNote.WhiskyId == whiskyId
                        select new Models.TastingNote
                        {
                            TastingNoteId = tastingNote.TastingNoteId,
                            WhiskyId = tastingNote.WhiskyId,
                            EventId = tastingNote.EventId,
                            Comment = tastingNote.Comment
                        };

            return items.ToList();
        }

        public Models.TastingNote InsertTastingNote(int whiskyId, int eventId, int memberId, string comment)
        {
            try
            {
                var tastingNote = new TastingNote();
                tastingNote.WhiskyId = whiskyId;
                tastingNote.EventId = eventId;
                tastingNote.MemberId = memberId;
                tastingNote.Comment = comment;
                tastingNote.InsertedDate = DateTime.Now;
                tastingNote.UpdatedDate = DateTime.Now;

                Insert(tastingNote);

                CommitChanges();

                return new Models.TastingNote
                {
                    TastingNoteId = tastingNote.TastingNoteId,
                    WhiskyId = tastingNote.WhiskyId,
                    EventId = tastingNote.EventId,
                    Comment = tastingNote.Comment
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateTastingNote(int tastingNoteId, int whiskyId, int eventId, int memberId, string comment)
        {
            try
            {
                var tastingNote = GetOne<TastingNote, int>(tastingNoteId);
                tastingNote.WhiskyId = whiskyId;
                tastingNote.EventId = eventId;
                tastingNote.MemberId = memberId;
                tastingNote.Comment = comment;
                tastingNote.UpdatedDate = DateTime.Now;

                Update(tastingNote);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteTastingNote(int tastingNoteId)
        {
            try
            {
                var tastingNote = new TastingNote();
                tastingNote.TastingNoteId = tastingNoteId;

                Delete(tastingNote);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public byte[] GetTastingNoteImage(int tastingNoteId)
        {
            var tastingNote = GetOne<TastingNote, int>(tastingNoteId);

            return tastingNote.Image;
        }

        public bool UpdateTastingNoteImage(int tastingNoteId, byte[] image)
        {
            try
            {
                var tastingNote = GetOne<TastingNote, int>(tastingNoteId);
                tastingNote.Image = image;
                tastingNote.UpdatedDate = DateTime.Now;

                Update(tastingNote);

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
