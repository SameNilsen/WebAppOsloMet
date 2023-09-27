using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppOsloMet.DAL;
using WebAppOsloMet.Models;
using WebAppOsloMet.ViewModels;

//  Await, async, Task is for async 

namespace WebAppOsloMet.Controllers;

public class ItemController : Controller
{
    //private readonly ItemDbContext _itemDbContext;  //  Uten repository pattern
    private readonly IItemRepository _itemRepository;

    public ItemController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    //public List<Order> OrderConsole()
    //{
    //    return _itemDbContext.Orders.ToList();
    //}

    public async Task<IActionResult> Table()
    {
        //var items = GetItems();    //  Gamle metode (uten db)
        //List<Item> items = await _itemDbContext.Items.ToListAsync();  //  Uten repo pattern
        var items = await _itemRepository.GetAll();
        var itemListViewModel = new ItemListViewModel(items, "Table");
        return View(itemListViewModel);
    }

    public async Task<IActionResult> Grid()
    {
        //var items = GetItems();    //  Gamle metode (uten db)
        var items = await _itemRepository.GetAll();
        var itemListViewModel = new ItemListViewModel(items, "Grid");
        return View(itemListViewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        //List<Item> items = _itemDbContext.Items.ToList();
        //var item = await _itemDbContext.Items.FirstOrDefaultAsync(i => i.ItemID == id);
        var item = await _itemRepository.GetItemById(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Item item)
    {
        if (ModelState.IsValid)
        {
            //_itemDbContext.Items.Add(item);
            //await _itemDbContext.SaveChangesAsync();
            await _itemRepository.Create(item);
            return RedirectToAction(nameof(Table));
        }
        return View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        //var item = await _itemDbContext.Items.FindAsync(id);
        var item = await _itemRepository.GetItemById(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Item item)
    {
        if ((ModelState.IsValid))
        {
            //_itemDbContext.Items.Update(item);
            //await _itemDbContext.SaveChangesAsync();
            await _itemRepository.Update(item);
            return RedirectToAction(nameof(Table));
        }
        return View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        //var item = await _itemDbContext.Items.FindAsync(id);
        var item = await _itemRepository.GetItemById(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        //var item = await _itemDbContext.Items.FindAsync(id);
        //if (item == null)
        //{
        //    return NotFound();
        //}
        //_itemDbContext.Items.Remove(item);
        //await _itemDbContext.SaveChangesAsync();
        await _itemRepository.Delete(id);
        return RedirectToAction(nameof(Table));
    }

    //public IActionResult Table()
    //{
    //    var items = GetItems();
    //    ViewBag.CurrentViewName = "Table";
    //    return View(items);
    //}

    //public IActionResult Grid()
    //{
    //    var items = GetItems();
    //    ViewBag.CurrentViewName = "Grid";
    //    return View(items);
    //}

    public List<Item> GetItems()
    {
        var items = new List<Item>();
        var item1 = new Item
        {
            ItemID = 1,
            Name = "coke",
            Price = 1340,
            Description = "itsa drink, cokk",
            ImageUrl = "/images/coke.jpg"
        };
        var item2 = new Item
        {
            ItemID = 2,
            Name = "Pizza",
            Price = 10,
            Description = "Good PIzz",
            ImageUrl = "/images/pizza.jpg"
        };
        //var item3 = new Item { };
        items.Add(item1);
        items.Add(item2);
        //ViewBag.CurrentViewName = "List of items:";
        return items;
    }
}

