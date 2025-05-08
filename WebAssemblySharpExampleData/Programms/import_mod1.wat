;; This module exports a trivial `times2` function.
;;
(module
    (func (export "times2") (param i32) (result i32)
        (i32.add (local.get 0) (local.get 0))
    )
)