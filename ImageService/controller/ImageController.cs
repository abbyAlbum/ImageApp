using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModel m_model;                      // The Modal Object
        private Dictionary<int, ICommand> commands;

        /// <summary>
        /// Creates the ImageController.
        /// Param: IImagerServiceModel.
        /// Creates the command - the NewFileCommand.
        /// </summary>
        public ImageController(IImageServiceModel model)
        {
            m_model = model;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>
            {
                { 0, new NewFileCommand(m_model) }
            };

        }

        /// <summary>
        /// Executes the command inputted.
        /// Param: int - the id of the command, string - args, bool - result.
        /// </summary>
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            return commands[commandID].Execute(args, out resultSuccesful); 
        }
    }
}

