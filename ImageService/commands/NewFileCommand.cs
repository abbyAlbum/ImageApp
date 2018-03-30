using ImageService.Infrastructure;
using ImageService.Modal;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModel m_model;

        public NewFileCommand(IImageServiceModel model)
        {
            m_model = model;            // Storing the Modal
        }

        public string Execute(string[] args, out bool result)
        {
			// The String Will Return the New Path if result = true, and will return the error message
        }
    }
}
