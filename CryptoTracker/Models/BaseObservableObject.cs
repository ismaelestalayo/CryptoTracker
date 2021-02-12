using Microsoft.Toolkit.Mvvm.ComponentModel;
/// <summary>
/// Special thanks to Panda-Sharp
/// https://github.com/Panda-Sharp/Yugen.Toolkit/blob/8739db96ae7f861e4c167985eb0db509d1f86b7f/Yugen.Toolkit.Uwp.Samples/ObservableObjects/PersonObservableObject.cs
/// </summary>

namespace CryptoTracker.Models {
	/// <summary>
	/// A base class for Generic objects of which the properties must be observable.
	/// </summary>
	public abstract class BaseObservableObject<T> : ObservableObject where T : class, new() {
        /// <summary>
        /// A model wrapped in every ObservableObject(T) object
        /// </summary>
        protected T Model { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObject"/> class.
        /// </summary>
        /// <param name="model">The wrapped model</param>
        public BaseObservableObject(T model = null) {
            Model = model ?? new T();
        }

        /// <summary>
        /// Creates a T object from observableObject. 
        /// </summary>
        /// <param name="observableObject">the observableObject</param>
        public static implicit operator T(BaseObservableObject<T> observableObject) =>
            observableObject.Model;
    }
}