using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{

    [Route("api/[controller]")]    //ApiController必须使用特性路由,将 [controller] 替换为控制器的名称
    [ApiController]                      //此属性指示该控制器响应Web Api请求。
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
            if(_context.TodoItems.Count()==0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "item1" });
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// GetTodoItems 和 GetTodoItem 方法的返回类型是 ActionResult<T> 类型。 ASP.NET Core 自动将对象序列化为 JSON，并将 JSON 写入响应消息的正文中。 在假设没有未经处理的异常的情况下，此返回类型的响应代码为 200。
        ///  ActionResult 返回类型可以表示大范围的 HTTP 状态代码。 例如，GetTodoItem 可以返回两个不同的状态值：如果没有任何项与请求的 ID 匹配，则该方法将返回 404 NotFound 错误代码。否则，此方法将返回具有 JSON 响应正文的 200。 返回 item 则产生 HTTP 200 响应
        /// </summary>


                //[HttpGet] 属性表示响应 HTTP GET 请求的方法
                //Get:api/Todo
                [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        //如果 [HttpGet] 属性具有路由模板（例如 [HttpGet("products")]），则将它追加到路径
        //Get:api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem= await _context.TodoItems.FindAsync(id);
            if(todoItem == null)
            {
                return NotFound();
            }
            return todoItem;
        }

        /// <summary>
        /// CreatedAtAction 方法如果成功，则返回 HTTP 201 状态代码。 HTTP 201 是在服务器上创建新资源的 HTTP POST 方法的标准响应。将 Location 标头添加到响应。 Location 标头指定新建的待办事项的 URI。 有关详细信息，请参阅创建的 10.2.2 201。引用 GetTodoItem 操作以创建 Location 标头的 URI。 C# nameof 关键字用于避免在 CreatedAtAction 调用中硬编码操作名称。
        /// </summary>

        //Post:api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }

        //Put:api/Todo/4
        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoItem(long id,TodoItem item)
        {
            if(id!=item.Id)
            {
                return BadRequest();
            }
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(long id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if(item==null)
            {
                return NotFound();
            }
            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }





    }
}
