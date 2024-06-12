using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Data;
using MovieDatabase.Models;

namespace MovieDatabase.Controllers
{
    public class SubcommentsController : Controller
    {
        private readonly MovieDatabaseContext _context;

        public SubcommentsController(MovieDatabaseContext context)
        {
            _context = context;
        }

        // GET: Subcomments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subcomment.ToListAsync());
        }

        // GET: Subcomments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcomment = await _context.Subcomment
                .FirstOrDefaultAsync(m => m.id == id);
            if (subcomment == null)
            {
                return NotFound();
            }

            return View(subcomment);
        }

        // GET: Subcomments/Create
        public IActionResult Create()
        {
            ViewBag.commentVB = new SelectList(_context.Comment, "id", "content");
            ViewBag.userVB = new SelectList(_context.User, "Id", "UserName");
            return View();
        }

        // POST: Subcomments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,comment_id,user_id,content,time,is_blocked")] Subcomment subcomment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subcomment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subcomment);
        }

        // GET: Subcomments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcomment = await _context.Subcomment.FindAsync(id);
            if (subcomment == null)
            {
                return NotFound();
            }

            ViewBag.commentVB = new SelectList(_context.Comment, "id", "content");
            ViewBag.userVB = new SelectList(_context.User, "Id", "UserName");
            return View(subcomment);
        }

        // POST: Subcomments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,comment_id,user_id,content,time,is_blocked")] Subcomment subcomment)
        {
            if (id != subcomment.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subcomment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubcommentExists(subcomment.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subcomment);
        }

        // GET: Subcomments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcomment = await _context.Subcomment
                .FirstOrDefaultAsync(m => m.id == id);
            if (subcomment == null)
            {
                return NotFound();
            }

            return View(subcomment);
        }

        // POST: Subcomments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcomment = await _context.Subcomment.FindAsync(id);
            if (subcomment != null)
            {
                _context.Subcomment.Remove(subcomment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubcommentExists(int id)
        {
            return _context.Subcomment.Any(e => e.id == id);
        }
    }
}
