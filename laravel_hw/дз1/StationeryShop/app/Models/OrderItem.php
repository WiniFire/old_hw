<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class OrderItem extends Model
{
    public $timestamps = false;
    protected $fillable = ['product_id', 'name', 'price_cents', 'quantity'];

    public function formattedSubtotal(): string
    {
        return number_format($this->price_cents * $this->quantity / 100, 2, ',', ' ').' грн';
    }
}
