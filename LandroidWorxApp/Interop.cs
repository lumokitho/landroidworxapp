using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandroidWorxApp
{
    public class Interop
    {
        private readonly IJSRuntime _jsRuntime;

        public Interop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetElementByName(string name)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>(
                    "interop.getElementByName",
                    name);
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task SubmitForm(string path, object fields)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync(
                    "interop.submitForm",
                    path, fields);
            }
            catch
            {
            }
        }
    }
}
