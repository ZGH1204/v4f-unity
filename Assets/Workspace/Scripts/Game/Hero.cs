// <copyright file="PuppetWrapper.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using V4F.Puppets;

namespace V4F.Game
{

    public class Hero
    {
        #region Fields
        private Puppet _puppet;
        private Location _location;
        #endregion

        #region Properties
        public Puppet puppet
        {
            get { return _puppet; }
        }

        public Location location
        {
            get { return _location; }
            set { _location = value; }
        }
        #endregion

        #region Constructors
        public Hero(Puppet puppet, Location location)
        {
            _puppet = puppet;
            _location = location;
        }
        #endregion
    }

}
