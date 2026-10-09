<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasMany;

class Order extends Model
{
    protected $fillable = ['customer_name', 'email', 'phone', 'address', 'total_cents', 'status', 'public_token'];

    public function items(): HasMany
    {
        return $this->hasMany(OrderItem::class);
    }

    public function formattedTotal(): string
    {
        return number_format($this->total_cents / 100, 2, ',', ' ').' грн';
    }

    public function statusLabel(): string
    {
        return ['new' => 'Новый', 'processing' => 'В обработке', 'completed' => 'Завершён'][$this->status] ?? $this->status;
    }
}
