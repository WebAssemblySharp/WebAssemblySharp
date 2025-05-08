;; This module imports `times2` and exports `twiceplus5`.
;;
(module
	(import "mod1" "times2" (func $times2 (param i32) (result i32)))

    (func (export "twiceplus5") (param i32) (result i32)
        (i32.add (call $times2 (local.get 0)) (i32.const 5))
    )
)