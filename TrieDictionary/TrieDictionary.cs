namespace TrieDictionary;

/// <summary>
/// An implementation of a dictionary using a trie
/// </summary>
/// <typeparam name="T">The type of the values in the dictionary</typeparam>
public class TrieDictionary<T> {
	private readonly TrieDictionary<T>[] _children = new TrieDictionary<T>[256];
	private int _amountChildren = 0;
	private NullableT<T>? _value;

	/// <summary>
	/// See: <see cref="Get"/> and <see cref="Insert"/>
	/// </summary>
	/// <param name="key">The key to be passed to <see cref="Get"/> and <see cref="Insert"/></param>
	public T this[string key] {
		get => Get(key);
		set => Insert(key, value);
	}

	/// <summary>
	/// Checks if a key is present in the dictionary
	/// </summary>
	/// <param name="key">The key to look up</param>
	/// <returns>true if the key is present, false if not</returns>
	public bool Contains(string key) {
		short charCode = (short) key[0];
		if (charCode >= _children.Length)
			return false;

		if (_children[charCode] == null!)
			return false;

		TrieDictionary<T> t = _children[charCode];

		if (key.Length > 1)
			return t.Contains(key.Substring(1));

		if (t._value != null)
			return true;

		return false;
	}
	
	/// <summary>
	/// Gets the value from the dictionary which is connected to the specified key
	/// </summary>
	/// <param name="key">The key to look up</param>
	/// <returns>The value connected to the key</returns>
	/// <exception cref="ArgumentException">Is thrown when the key is not utf-8</exception>
	/// <exception cref="KeyNotFoundException">Is thrown when the key is not present in the dictionary</exception>
	public T Get(string key) {
		short charCode = (short) key[0];
		if (charCode >= _children.Length)
			throw new ArgumentException("invalid key");

		if (_children[charCode] == null!)
			throw new KeyNotFoundException();

		TrieDictionary<T> t = _children[charCode];

		if (key.Length > 1)
			return t.Get(key.Substring(1));

		if (t._value != null)
			return t._value.Value;

		throw new KeyNotFoundException();
	}

	/// <summary>
	/// Inserts a key into the dictionary and connects a value to it
	/// </summary>
	/// <param name="key">The key to be inserted into the dictionary</param>
	/// <param name="value">The value to connect to the key</param>
	/// <exception cref="ArgumentException">Is thrown when the key is not utf-8</exception>
	public void Insert(string key, T value) {
		short charCode = (short) key[0];
		if (charCode >= _children.Length)
			throw new ArgumentException("invalid key");
		
		TrieDictionary<T> t;
		if (_children[charCode] == null!) {
			t = new TrieDictionary<T>();
			_children[charCode] = t;
			_amountChildren++;
		} else {
			t = _children[charCode];
		}

		if (key.Length > 1)
			t.Insert(key.Substring(1), value);
		else
			t._value = new NullableT<T> {Value = value};
	}

	/// <summary>
	/// Removes a key and its connected value from the dictionary
	/// </summary>
	/// <param name="key">The key to be removed from the dictionary</param>
	/// <exception cref="ArgumentException">Is thrown when the key is not utf-8</exception>
	/// <exception cref="KeyNotFoundException">Is thrown when the key is not present in the dictionary</exception>
	public void Remove(string key) {
		short charCode = (short) key[0];
		if (charCode >= _children.Length)
			throw new ArgumentException("invalid key");

		if (_children[charCode] == null!)
			throw new KeyNotFoundException();

		TrieDictionary<T> t = _children[charCode];

		if (key.Length > 1) {
			t.Remove(key.Substring(1));
			if (t._amountChildren == 0 && t._value == null) {
				_children[charCode] = null!;
				_amountChildren--;
			}
		} else if (t._value != null) {
			if (t._amountChildren == 0) {
				_children[charCode] = null!;
				_amountChildren--;
			} else {
				t._value = null;
			}
		} else {
			throw new KeyNotFoundException();
		}
	}

	/// <summary>
	/// Gives a set with all the keys in the dictionary
	/// </summary>
	/// <returns>All the keys present in the dictionary</returns>
	public string[] GetKeySet() => GetKeySet("");

	/// <summary>
	/// Gives a set with all the keys in the dictionary
	/// </summary>
	/// <param name="key">The concatenation of all the chars of the current node's parents and the current node's char</param>
	/// <returns>All the keys present in the dictionary</returns>
	private string[] GetKeySet(string key) {
		HashSet<string> set = new HashSet<string>();
		for (int i = 0; i < _children.Length; i++)
			if (_children[i] != null!)
				set.UnionWith(_children[i].GetKeySet(key + (char) i));

		if (_value != null)
			set.Add(key);

		return set.ToArray();
	}
}