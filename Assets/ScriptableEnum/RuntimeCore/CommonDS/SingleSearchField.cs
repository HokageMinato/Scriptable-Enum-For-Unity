using System;

namespace ScriptableEnumSystem.CommonDS
{

    public class SingleSearchField<T>
    {
        #region PRIVATE_VARS
        private Func<T> _resolver;
        private Func<bool> _discardCacheCondition;
        private bool isResolved = false;
        private T _value = default;
        #endregion


        #region PRIVATE_PROPERTIES
        public T ResolvedValue
        {
            get
            {
                CheckForResultDiscard();
                ResolveValue();

                return _value;
            }

        }
        #endregion


        #region CONSTRUCTOR
        public SingleSearchField(Func<T> resolver, Func<bool> discardCacheCondition = null)
        {
            Init(resolver, discardCacheCondition);
        }

        public void Init(Func<T> resolver, Func<bool> discardCacheCondition)
        {
            _resolver = resolver;
            _discardCacheCondition = discardCacheCondition;
        }
        #endregion


        #region PRIVATE_METHODS
        private void ResolveValue()
        {
            if (isResolved)
                return;
         
            _value = _resolver();
            isResolved = true;
        }

        private void CheckForResultDiscard()
        {
            if (_discardCacheCondition == null)
                return;

            if (_discardCacheCondition())
                DiscardCache();
        }
        #endregion

        #region PUBLIC_METHODS
        public void DiscardCache()
        {
            _value = default;
            isResolved = false;
        }
        #endregion


    }

}

