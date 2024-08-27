namespace IntermediateAPI.Models
{
    public class AllowedLogins
    {
        private IEnumerable<string>? _loginTypes;

        public string Domain { get; set; }
        public IEnumerable<string> LoginTypes
        {
            get
            {
                if (_loginTypes != null && _loginTypes.Any())
                {
                    return _loginTypes;
                }
                else
                {
                    return new List<string>() { Constants.Local };
                }
            }

            set => _loginTypes = value;
        }
        public void RemoveLoginType(string loginType)
        {
            if (_loginTypes != null)
            {
                _loginTypes = _loginTypes.Where(x => x != loginType);
            }
        }
        public void AddLoginType(string loginType)
        {
            if (!HasLogin(loginType) && _loginTypes != null)
            {
                _loginTypes = _loginTypes.Append(loginType);
            }
            else
            {
                _loginTypes = new List<string>() { loginType };
            }
        }
        public bool HasLogin(string loginType)
        {
            if (_loginTypes == null)
            {
                return false;
            }
            return _loginTypes.Contains(loginType);
        }
    }
}
