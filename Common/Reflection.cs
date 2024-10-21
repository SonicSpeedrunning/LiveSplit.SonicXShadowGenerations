using System;
using System.Reflection;

namespace Helper.Common.Reflection;

/// <summary>
/// The <see cref="Reflection"/> class provides extension methods for interacting with objects using reflection.
/// It includes methods to get and set fields and properties, as well as invoking methods dynamically.
/// </summary>
public static partial class Reflection
{
    /// <summary>
    /// Gets the value of a property from an object using reflection.
    /// </summary>
    /// <typeparam name="T">The expected type of the property's value.</typeparam>
    /// <param name="obj">The object to read the property value from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns>The value of the specified property.</returns>
    public static T GetPropertyValue<T>(this object obj, string propertyName)
    {
        PropertyInfo property = GetMember<PropertyInfo>(obj, propertyName);
        return (T)property.GetValue(obj);
    }

    /// <summary>
    /// Sets the value of a property on an object using reflection.
    /// </summary>
    /// <param name="obj">The object on which to set the property value.</param>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value to assign to the property.</param>
    public static void SetPropertyValue(this object obj, string propertyName, object value)
    {
        PropertyInfo property = GetMember<PropertyInfo>(obj, propertyName);
        if (!property.CanWrite)
            throw new InvalidOperationException($"Property '{propertyName}' is read-only.");

        property.SetValue(obj, value);
    }

    /// <summary>
    /// Gets the value of a field from an object using reflection.
    /// </summary>
    /// <typeparam name="T">The expected type of the field's value.</typeparam>
    /// <param name="obj">The object to read the field value from.</param>
    /// <param name="fieldName">The name of the field to retrieve.</param>
    /// <returns>The value of the specified field.</returns>
    public static T GetFieldValue<T>(this object obj, string fieldName)
    {
        FieldInfo field = GetMember<FieldInfo>(obj, fieldName);
        return (T)field.GetValue(obj);
    }

    /// <summary>
    /// Sets the value of a field on an object using reflection.
    /// </summary>
    /// <param name="obj">The object on which to set the field value.</param>
    /// <param name="fieldName">The name of the field to set.</param>
    /// <param name="value">The value to assign to the field.</param>
    public static void SetFieldValue(this object obj, string fieldName, object value)
    {
        FieldInfo field = GetMember<FieldInfo>(obj, fieldName);
        field.SetValue(obj, value);
    }

    /// <summary>
    /// Invokes a method on an object using reflection.
    /// </summary>
    /// <param name="obj">The object on which to invoke the method.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="parameters">The parameters to pass to the method, if any.</param>
    /// <returns>The result of the method invocation.</returns>
    public static object InvokeMethod(this object obj, string methodName, params object[] parameters)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (string.IsNullOrWhiteSpace(methodName))
            throw new ArgumentNullException(nameof(methodName));

        // Get the type of the object and retrieve the method info based on the method name.
        Type type = obj.GetType();
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // Throw an exception if the method could not be found.
        if (method == null)
            throw new ArgumentException($"Method '{methodName}' not found on type '{type.FullName}'.");

        // Invoke the method on the object and return the result.
        return method.Invoke(obj, parameters);
    }

    /// <summary>
    /// Retrieves a member (either a property or field) from an object using reflection.
    /// </summary>
    /// <typeparam name="T">The type of the member to retrieve (PropertyInfo or FieldInfo).</typeparam>
    /// <param name="obj">The object from which to retrieve the member.</param>
    /// <param name="memberName">The name of the member to retrieve.</param>
    /// <returns>The member information of the specified member (PropertyInfo or FieldInfo).</returns>
    /// <exception cref="ArgumentException">Thrown if the member is not found on the object.</exception>
    private static T GetMember<T>(object obj, string memberName) where T : MemberInfo
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (string.IsNullOrWhiteSpace(memberName))
            throw new ArgumentNullException(nameof(memberName));

        // Get the type of the object and search for the member (either a property or a field).
        Type type = obj.GetType();
        MemberInfo member = typeof(T) == typeof(PropertyInfo)
            ? type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            : type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // Throw an exception if the member could not be found.
        if (member == null)
            throw new ArgumentException($"{typeof(T).Name} '{memberName}' not found on type '{type.FullName}'.");

        // Return the member information.
        return (T)member;
    }
}
