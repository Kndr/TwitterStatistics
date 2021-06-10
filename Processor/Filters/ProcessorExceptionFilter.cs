using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Processor.Filters
{
    public class ProcessorExceptionFilter: IExceptionFilter
    {
        public ProcessorExceptionFilter()
        {

        }

        public void OnException(ExceptionContext ctx)
        {
            var result = new StatusCodeResult(500);

            ctx.Result = result;

        }
    }
}
