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
        /// <summary>
		/// Creates the NewFileCommand.
        /// Param: the IImageServiceModel
		/// </summary>
        public NewFileCommand(IImageServiceModel model)
        {
            m_model = model;            // Storing the Modal
        }
        /// <summary>
        /// Executes the new file command - i.e adds the file.
        /// Param: string - args, bool - the result.
        /// </summary>
        public string Execute(string[] args, out bool result)
        {
             return m_model.AddFile(args[0], out result); // The String Will Return the New Path if result = true, and will return the error message
            
        }
    }
}
