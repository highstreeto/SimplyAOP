# SimplyAOP.Example

This a simple example showing of the capabilities of SimplyAOP.

It includes (implemented as advices):
* Console trace of method invocations
* Stopwatch for methods
* Ambient transactions handling
* Caching results of methods
* (Very basic) fuzzing

## Exemplary Output

```
Method Random() begin
 TX Start
  Random(...) took 00:00:00.0007249
 TX Complete
Method Random() = 1718877098 end
Method Random() begin
 TX Start
  Random(...) took 00:00:00.0000028
 TX Complete
Method Random() = 369473097 end
Method Execute(-1) begin
 TX Start
  Execute(...) took 00:00:00.0007780
 TX Rollback
Method Execute() ! ArgumentOutOfRangeException: N must be positive! Parameter name: n Actual value was -1. end
Caught ArgumentOutOfRangeException
Method Execute(10) begin
 TX Start
  Crunching numbers ... Done!
  Execute(...) took 00:00:04.8166876
 TX Complete
Method Execute() = 5000 end
Method Execute(1) begin
 TX Start
  Crunching numbers ... Done!
  Execute(...) took 00:00:00.3365715
 TX Complete
Method Execute() = 500 end
--------------- Begin Caching ---------------
Method Execute(10) begin
 TX Start
  Crunching numbers ... Done!
  Execute(...) took 00:00:00.0180287
 TX Complete
Method Execute() = 5000 end
Method Execute(10) begin
 TX Start
  Execute(...) took 00:00:00.0005195
 TX Complete
Method Execute() = 5000 end
Method Execute(10) begin
 TX Start
  Execute(...) took 00:00:00.0000122
 TX Complete
Method Execute() = 5000 end
Method Execute(1) begin
 TX Start
  Crunching numbers ... Done!
  Execute(...) took 00:00:00.1613955
 TX Complete
Method Execute() = 500 end
Method Execute(1) begin
 TX Start
  Execute(...) took 00:00:00.0000018
 TX Complete
Method Execute() = 500 end
Method Execute(1) begin
 TX Start
  Execute(...) took 00:00:00.0000005
 TX Complete
Method Execute() = 500 end
--------------- End Caching ---------------
Method Sum((1, -1)) begin
 TX Start
  Summing numbers ... Done!
  Sum(...) took 00:00:00.0011085
 TX Complete
Method Sum() = 0 end
Method Sum((100, 0)) begin
 TX Start
  Summing numbers ... Done!
  Sum(...) took 00:00:00.0000146
 TX Complete
Method Sum() = 100 end
--------------- Begin Fuzz Sum() ---------------
Method Sum((-131925008, 644590629)) begin
  Summing numbers ... Done!
Method Sum() = 512665621 end
Method Sum((-789824446, 1559702337)) begin
  Summing numbers ... Done!
Method Sum() = 769877891 end
Method Sum((-2049114219, 1156648783)) begin
  Summing numbers ... Done!
Method Sum() = -892465436 end
Method Sum((-430240055, -2035922898)) begin
  Summing numbers ... Done!
Method Sum() = 1828804343 end
Method Sum((1063595435, -1972533665)) begin
  Summing numbers ... Done!
Method Sum() = -908938230 end
Method Sum((169196105, 1427683033)) begin
  Summing numbers ... Done!
Method Sum() = 1596879138 end
Method Sum((-619554199, -1868161821)) begin
  Summing numbers ... Done!
Method Sum() = 1807251276 end
Method Sum((1161238448, -462891494)) begin
  Summing numbers ... Done!
Method Sum() = 698346954 end
Method Sum((-1683483352, 1143686046)) begin
  Summing numbers ... Done!
Method Sum() = -539797306 end
Method Sum((873205378, 661470980)) begin
  Summing numbers ... Done!
Method Sum() = 1534676358 end
Method Sum((-2111175336, -1297704968)) begin
  Summing numbers ... Done!
Method Sum() = 886086992 end
Method Sum((787906804, -88358001)) begin
  Summing numbers ... Done!
Method Sum() = 699548803 end
Method Sum((778022722, 1032864080)) begin
  Summing numbers ... Done!
Method Sum() = 1810886802 end
Method Sum((-233112375, -1565324794)) begin
  Summing numbers ... Done!
Method Sum() = -1798437169 end
Method Sum((-390614782, 313111717)) begin
  Summing numbers ... Done!
Method Sum() = -77503065 end
Method Sum((749929519, -783560894)) begin
  Summing numbers ... Done!
Method Sum() = -33631375 end
--------------- End Fuzz ---------------
```