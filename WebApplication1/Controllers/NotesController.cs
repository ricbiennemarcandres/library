using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    public class NotesController : Controller
    {
        private readonly NoteRepository _noteRepository;

        public NotesController(NoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        // Display all notes
        public async Task<IActionResult> Index()
        {
            var notes = await _noteRepository.GetAllAsync();
            return View(notes);
        }

        // Show the Create page
        public IActionResult Create()
        {
            return View();
        }

        // Save a new note
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            if (ModelState.IsValid)
            {
                await _noteRepository.AddAsync(note);
                return RedirectToAction(nameof(Index));
            }

            return View(note);
        }

        // Show the Edit page
        public async Task<IActionResult> Edit(long id)
        {
            var note = await _noteRepository.GetByIdAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // Save changes to a note
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Note note)
        {
            if (id != note.NoteId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var existingNote = await _noteRepository.GetByIdAsync(id);

                if (existingNote == null)
                {
                    return NotFound();
                }

                await _noteRepository.UpdateAsync(note);

                return RedirectToAction(nameof(Index));
            }

            return View(note);
        }

        // Show the Delete confirmation page
        public async Task<IActionResult> Delete(long id)
        {
            var note = await _noteRepository.GetByIdAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // Delete the selected note
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var note = await _noteRepository.GetByIdAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            await _noteRepository.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}