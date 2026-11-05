using Npgsql;
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _members;

    public MembersController(MemberRepository members)
    {
        _members = members;
    }

    // GET /Members : display the list of members.
    public async Task<IActionResult> Index()
    {
        var members = await _members.GetAllAsync();

        return View(members);
    }

    // GET /Members/Create : display an empty form.
    public IActionResult Create()
    {
        return View(new Member());
    }

    // POST /Members/Create : save the new member.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        await _members.AddAsync(member);

        return RedirectToAction(nameof(Index));
    }

    // GET /Members/Edit/5 : display the member's information.
    public async Task<IActionResult> Edit(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST /Members/Edit/5 : save the edited information.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        await _members.UpdateAsync(member);

        return RedirectToAction(nameof(Index));
    }

    // GET /Members/Delete/5 : ask for confirmation.
    public async Task<IActionResult> Delete(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST /Members/Delete/5 : the delete was confirmed.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            // 1. Delete the member.
        }
        catch (PostgresException ex) when (/* 2. the database refused because of a foreign key */)
        {
            // 3. Load the member again, to show them on the page.
            if (/* 4. there is no such member */)
            {
                // 5. Answer "not found".
            }
            // 6. Set the message the page will show.
            // 7. Show the Delete page again, with the member.
        }

        // 8. The member was deleted: go back to the list.
    }
}