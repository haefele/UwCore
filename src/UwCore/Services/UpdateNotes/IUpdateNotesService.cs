namespace UwCore.Services.UpdateNotes
{
    public interface IUpdateNotesService
    {
        bool HasSeenUpdateNotes();

        void MarkUpdateNotesAsSeen();

        void Clear();
    }
}