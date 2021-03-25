﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BooksManager.Data;
using BooksManager.Models;

namespace BooksManager.Controllers
{
    public class ReadLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReadLogsRepository readLogsRepository;

        public ReadLogsController(ApplicationDbContext context, IReadLogsRepository readLogsRepository)
        {
            _context = context;
            this.readLogsRepository = readLogsRepository;
        }

        // GET: ReadLogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ReadLogs.Include(r => r.Book);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ReadLogs/Details/5
        public IActionResult Details(int id)
        {
            var readLog = readLogsRepository.GetLogById(id);
            if (readLog == null)
            {
                return NotFound();
            }

            return View(readLog);
        }

        // GET: ReadLogs/Create
        public IActionResult Create(int id, string bookName)
        {
            ViewData["BookId"] = id;
            ViewData["BookName"] = bookName;
            return View();
        }

        // POST: ReadLogs/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ReadLogId,PageNumber,LogDate,Note")] ReadLog readLog, int id, string bookName)
        {
            if (ModelState.IsValid)
            {
                readLog.BookId = id;
                var logCreated = readLogsRepository.AddLog(readLog);
                if (logCreated)
                {
                    return RedirectToAction("Detail", "Book", new { id });
                }
                else
                {
                    ModelState.AddModelError("log-validation", "There was an error validating the log. Check either the page number or the date.");
                }
            }
            //ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Name", readLog.BookId);
            ViewData["BookId"] = id;
            ViewData["BookName"] = bookName;
            return View(readLog);
        }

        // GET: ReadLogs/Edit/5
        public IActionResult Edit(int id)
        {

            var readLog = readLogsRepository.GetLogById(id);
            if (readLog == null)
            {
                return NotFound();
            }
            //ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Name", readLog.BookId);
            return View(readLog);
        }

        // POST: ReadLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ReadLogId,PageNumber,LogDate,Note,BookId")] ReadLog readLog)
        {
            if (id != readLog.ReadLogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    readLogsRepository.UpdateLog(readLog);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReadLogExists(readLog.ReadLogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Logs", "Book", new { id = readLog.BookId });
            }
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "Name", readLog.BookId);
            return View(readLog);
        }

        // GET: ReadLogs/Delete/5
        public IActionResult Delete(int id, int bookId)
        {

            readLogsRepository.DeleteLog(id);

            return RedirectToAction("Logs", "Book", new { id = bookId });
        }

        private bool ReadLogExists(int id)
        {
            return _context.ReadLogs.Any(e => e.ReadLogId == id);
        }
    }
}
