using CoreWCF.Channels;
using System;
using System.ComponentModel;
using System.Security.Authentication.ExtendedProtection;

namespace CoreWCF.Channels
{
    public partial class TcpTransportBindingElement : ConnectionOrientedTransportBindingElement
    {
        int listenBacklog;
        ExtendedProtectionPolicy extendedProtectionPolicy;

        public TcpTransportBindingElement() : base()
        {
            listenBacklog = TcpTransportDefaults.GetListenBacklog();
            extendedProtectionPolicy = ChannelBindingUtility.DefaultPolicy;
        }
        protected TcpTransportBindingElement(TcpTransportBindingElement elementToBeCloned) : base(elementToBeCloned)
        {
            listenBacklog = elementToBeCloned.listenBacklog;
            extendedProtectionPolicy = elementToBeCloned.ExtendedProtectionPolicy;
        }

        public int ListenBacklog
        {
            get
            {
                return listenBacklog;
            }

            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value",
                        SR.ValueMustBePositive));
                }

                listenBacklog = value;
            }
        }

        public override string Scheme
        {
            get { return "net.tcp"; }
        }

        public ExtendedProtectionPolicy ExtendedProtectionPolicy
        {
            get
            {
                return extendedProtectionPolicy;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (value.PolicyEnforcement == PolicyEnforcement.Always &&
                    !ExtendedProtectionPolicy.OSSupportsExtendedProtection)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new PlatformNotSupportedException(SR.ExtendedProtectionNotSupported));
                }

                extendedProtectionPolicy = value;
            }
        }

        public override BindingElement Clone()
        {
            return new TcpTransportBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(context));
            }
            // TODO: Decide whether to support DeliveryRequirementsAttribute
            //if (typeof(T) == typeof(IBindingDeliveryCapabilities))
            //{
            //    return (T)(object)new BindingDeliveryCapabilitiesHelper();
            //}
            else if (typeof(T) == typeof(ExtendedProtectionPolicy))
            {
                return (T)(object)ExtendedProtectionPolicy;
            }
            // TODO: Support ITransportCompressionSupport
            //else if (typeof(T) == typeof(ITransportCompressionSupport))
            //{
            //    return (T)(object)new TransportCompressionSupportHelper();
            //}
            else
            {
                return base.GetProperty<T>(context);
            }
        }
    }
}