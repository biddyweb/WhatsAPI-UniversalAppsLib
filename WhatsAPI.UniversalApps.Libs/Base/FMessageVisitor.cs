using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAPI.UniversalApps.Libs.Base
{
    public interface FMessageVisitor
    {
        void Audio(FMessage fMessage);

        void Contact(FMessage fMessage);

        void Image(FMessage fMessage);

        void Location(FMessage fMessage);

        void System(FMessage fMessage);

        void Undefined(FMessage fMessage);

        void Video(FMessage fMessage);
    }
}
