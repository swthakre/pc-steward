// SPDX-License-Identifier: GPL-3.0-or-later
namespace Steward.Core;
public sealed class RequestGate
{
    private long current;
    public long Next() => Interlocked.Increment(ref current);
    public bool IsCurrent(long request) => request == Interlocked.Read(ref current);
}
