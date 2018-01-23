using DdpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DdpClient.Models.Server;
using System.Reflection;

namespace Meteor.StressTest
{
    public class Collection<T> : Dictionary<string, T>, IDdpSubscriber<T>
    {
        private Type Type;

        public Collection()
        {
            this.Type = typeof(T);
        }

        public virtual void Added(SubAddedModel<T> added)
        {
            this.Add(added.Id, added.Object);
        }

        public virtual void AddedBefore(SubAddedBeforeModel<T> addedBefore) { }

        public virtual void Changed(SubChangedModel<T> changed)
        {
            if (!this.ContainsKey(changed.Id))
                return;

            var item = this[changed.Id];

            if (changed.Cleared != null)
            {
                foreach (string fieldName in changed.Cleared)
                    this.Type.GetProperty(fieldName).SetValue(item, null);
            }

            foreach (PropertyInfo property in this.Type.GetProperties())
            {
                if (changed.Object == null)
                    continue;

                object value = property.GetValue(changed.Object);
                if (value == null)
                    continue;

                property.SetValue(item, value);
            }
        }

        public virtual void MovedBefore(SubMovedBeforeModel movedBefore) { }

        public virtual void Removed(SubRemovedModel removed)
        {
            this.Remove(removed.Id);
        }
    }
}
