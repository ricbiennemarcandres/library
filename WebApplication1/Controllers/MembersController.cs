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
}