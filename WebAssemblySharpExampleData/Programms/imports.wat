(module
	(import "env" "times2" (func $times2 (param i32) (result i32)))

    (func (export "twiceplus5") (param i32) (result i32)
        (i32.add (call $times2 (local.get 0)) (i32.const 5))
    )
)