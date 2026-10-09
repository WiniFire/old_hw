<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\SoftDeletes;

class Product extends Model
{
    use SoftDeletes;

    protected $fillable = ['name', 'category', 'description', 'price_cents', 'stock'];

    protected function casts(): array
    {
        return ['price_cents' => 'integer', 'stock' => 'integer'];
    }

    public function formattedPrice(): string
    {
        return number_format($this->price_cents / 100, 2, ',', ' ').' грн';
    }
}
