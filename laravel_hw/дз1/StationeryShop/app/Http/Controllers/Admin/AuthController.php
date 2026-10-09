<?php

namespace App\Http\Controllers\Admin;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Validation\ValidationException;

class AuthController
{
    public function create()
    {
        if (auth()->user()?->is_admin) return redirect()->route('admin.dashboard');

        return view('admin.login');
    }

    public function store(Request $request)
    {
        $credentials = $request->validate(['email' => 'required|email', 'password' => 'required|string']);

        if (! Auth::attempt($credentials + ['is_admin' => true])) {
            throw ValidationException::withMessages(['email' => 'Неверный email или пароль.']);
        }

        $request->session()->regenerate();

        return redirect()->route('admin.dashboard');
    }

    public function destroy(Request $request)
    {
        Auth::logout();
        $request->session()->invalidate();
        $request->session()->regenerateToken();

        return redirect()->route('catalog.index');
    }
}
